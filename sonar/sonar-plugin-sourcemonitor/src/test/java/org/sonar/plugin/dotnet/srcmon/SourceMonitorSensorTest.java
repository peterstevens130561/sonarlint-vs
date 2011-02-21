/*
 * Maven and Sonar plugin for .Net
 * Copyright (C) 2010 Jose Chillan and Alexandre Victoor
 * mailto: jose.chillan@codehaus.org or alexvictoor@codehaus.org
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */

package org.sonar.plugin.dotnet.srcmon;

import static org.mockito.Matchers.*;
import static org.mockito.Mockito.*;
import static org.mockito.AdditionalMatchers.*;
import static org.junit.Assert.*;


import java.io.File;

import org.apache.commons.configuration.Configuration;
import org.apache.commons.lang.StringUtils;
import org.apache.maven.dotnet.commons.project.VisualStudioUtils;
import org.apache.maven.project.MavenProject;
import org.junit.Before;
import org.junit.Ignore;
import org.junit.Test;
import org.mockito.ArgumentMatcher;
import org.sonar.api.batch.SensorContext;
import org.sonar.api.measures.CoreMetrics;
import org.sonar.api.measures.Measure;
import org.sonar.api.resources.Project;
import org.sonar.api.resources.ProjectFileSystem;
import org.sonar.api.resources.Resource;
import org.sonar.plugin.dotnet.core.resource.CSharpFileLocator;
import org.sonar.plugin.dotnet.srcmon.SourceMonitorPluginHandler;
import org.sonar.plugin.dotnet.srcmon.SourceMonitorSensor;

public class SourceMonitorSensorTest {

  private SourceMonitorSensor sensor;
  private SourceMonitorPluginHandler pluginHandler;
  
  @Before
  public void setUp() {
    pluginHandler = mock(SourceMonitorPluginHandler.class);
    sensor = new SourceMonitorSensor(pluginHandler, new CSharpFileLocator());
  }
  
  
  @Test
  public void test() {
    // set up maven project
    MavenProject mvnProject = new MavenProject();
    mvnProject.setPackaging("sln");
    mvnProject.getProperties().put(VisualStudioUtils.VISUAL_SOLUTION_NAME_PROPERTY, "MessyTestSolution.sln");
    File pomFile 
      = new File("target/test-classes/solution/MessyTestSolution/pom.xml");
    mvnProject.setFile(pomFile);
    
    // set up sonar project
    Project project = mock(Project.class);
    when(project.getPom()).thenReturn(mvnProject);
    Configuration configuration =  mock(Configuration.class);
  
    when(project.getConfiguration()).thenReturn(configuration);
    ProjectFileSystem projectFileSystem = mock(ProjectFileSystem.class);
    when(project.getFileSystem()).thenReturn(projectFileSystem);
    when(projectFileSystem.getBuildDir()).thenReturn(new File("target/test-classes/solution/MessyTestSolution/target"));
    when(projectFileSystem.getBasedir()).thenReturn(new File("target/test-classes/solution/MessyTestSolution"));
    SensorContext context = mock(SensorContext.class);
   
    sensor.analyse(project, context);
    
    // verify that not measures
    // for file Money.cs are saved and measures
    // from Money.cs does not corrupt folder and assembly measures
    verify(context, atLeastOnce()).saveMeasure(argThat(new IsCorrectResource()), any(Measure.class));
    verify(context, never()).saveMeasure(eq(CoreMetrics.LINES) , gt(200d)); // Money.cs has more than 200 loc
  }

  public static class IsCorrectResource extends ArgumentMatcher<Resource<?>> {

    @Override
    public boolean matches(Object argument) {
      final boolean result;
      if (argument instanceof Resource<?>) {
        Resource<?> res = (Resource<?>) argument;
        result = !StringUtils.containsIgnoreCase(res.getLongName(), "money");
      } else {
        result = false;
      }
      return result; 
    }
   
 }
}
